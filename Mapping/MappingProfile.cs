using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryDetailsDto>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductShortDto>();

        CreateMap<Role, RoleDto>();
        CreateMap<Role, RoleDetailsDto>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));

        CreateMap<User, UserDto>();
        CreateMap<User, UserShortDto>();

        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));
        CreateMap<CartItem, CartItemDto>();

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<ProductReservation, ProductReservationDto>();

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore());
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore());

        CreateMap<CreateRoleDto, Role>()
            .ForMember(dest => dest.Users, opt => opt.Ignore());
        CreateMap<UpdateRoleDto, Role>()
            .ForMember(dest => dest.Users, opt => opt.Ignore());

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore());
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore());

        CreateMap<CreateCartDto, Cart>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore());
        CreateMap<UpdateCartDto, Cart>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore());

        CreateMap<CreateCartItemDto, CartItem>()
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
        CreateMap<UpdateCartItemDto, CartItem>()
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
        CreateMap<UpdateOrderDto, Order>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<CreateOrderItemDto, OrderItem>()
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
        CreateMap<UpdateOrderItemDto, OrderItem>()
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
    }
}
