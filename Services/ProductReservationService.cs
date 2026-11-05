using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class ProductReservationService : IProductReservationService
{
    private readonly IProductReservationRepository _reservations;
    private readonly IProductRepository _products;
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public ProductReservationService(
        IProductReservationRepository reservations,
        IProductRepository products,
        IUserRepository users,
        IMapper mapper)
    {
        _reservations = reservations;
        _products = products;
        _users = users;
        _mapper = mapper;
    }

    public async Task<(List<ProductReservationDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct)
    {
        var items = await _reservations.GetAllAsync(page, pageSize, ct);
        var total = await _reservations.CountAsync(ct);
        return (_mapper.Map<List<ProductReservationDto>>(items), total);
    }

    public async Task<ProductReservationDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var reservation = await _reservations.GetByIdAsync(id, ct);
        return reservation is null ? null : _mapper.Map<ProductReservationDto>(reservation);
    }

    public async Task<ProductReservationDto> CreateAsync(
        CreateProductReservationDto dto, CancellationToken ct)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException(
                "Количество для резервирования должно быть больше нуля.");

        var product = await _products.GetByIdAsync(dto.ProductId, ct)
            ?? throw new InvalidOperationException(
                $"Товар с Id {dto.ProductId} не найден.");

        _ = await _users.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException(
                $"Пользователь с Id {dto.UserId} не найден.");

        if (product.StockQuantity < dto.Quantity)
            throw new InvalidOperationException(
                $"Недостаточно товара \"{product.ProductName}\" на складе: " +
                $"есть {product.StockQuantity}, требуется {dto.Quantity}.");

        product.StockQuantity -= dto.Quantity;
        _products.Update(product);

        var reservation = new ProductReservation
        {
            ProductId = dto.ProductId,
            UserId = dto.UserId,
            Quantity = dto.Quantity,
            CreatedAt = DateTime.UtcNow
        };
        await _reservations.AddAsync(reservation);

        await _reservations.SaveChangesAsync();

        var saved = await _reservations.GetByIdAsync(reservation.Id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить только что созданный резерв.");
        return _mapper.Map<ProductReservationDto>(saved);
    }

    public async Task<ProductReservationDto?> ReleaseAsync(int id, CancellationToken ct)
    {
        var reservation = await _reservations.GetByIdAsync(id, ct);

        if (reservation is null || reservation.ClosedAt is not null)
            return null;

        var product = await _products.GetByIdAsync(reservation.ProductId, ct)
            ?? throw new InvalidOperationException(
                $"Товар с Id {reservation.ProductId} не найден.");

        product.StockQuantity += reservation.Quantity;
        _products.Update(product);

        reservation.ClosedAt = DateTime.UtcNow;
        _reservations.Update(reservation);

        await _reservations.SaveChangesAsync();

        return _mapper.Map<ProductReservationDto>(reservation);
    }
}
