using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<OrderDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct)
    {
        var orders = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<OrderDto>>(orders), total);
    }

    public async Task<List<OrderDto>> GetByUserAsync(int userId, CancellationToken ct)
    {
        var orders = await _repository.GetByUserAsync(userId, ct);
        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct)
    {
        _ = await _repository.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException(
                $"Пользователь с Id {dto.UserId} не найден.");

        var order = _mapper.Map<Order>(dto);
        order.CreatedAt = DateTime.UtcNow;

        await _repository.AddAsync(order);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(order.Id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить только что созданный заказ.");
        return _mapper.Map<OrderDto>(saved);
    }

    public async Task<OrderDto?> UpdateAsync(
        int id, UpdateOrderDto dto, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        if (order is null) return null;

        _ = await _repository.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException(
                $"Пользователь с Id {dto.UserId} не найден.");

        _mapper.Map(dto, order);

        _repository.Update(order);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённый заказ.");
        return _mapper.Map<OrderDto>(saved);
    }

    public async Task<OrderDto?> PatchAsync(
        int id, PatchOrderDto dto, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        if (order is null) return null;

        if (dto.UserId is not null)
        {
            _ = await _repository.FindUserAsync(dto.UserId.Value, ct)
                ?? throw new InvalidOperationException(
                    $"Пользователь с Id {dto.UserId} не найден.");
            order.UserId = dto.UserId.Value;
        }

        _repository.Update(order);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённый заказ.");
        return _mapper.Map<OrderDto>(saved);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        if (order is null) return false;

        _repository.Remove(order);
        await _repository.SaveChangesAsync();
        return true;
    }
}
