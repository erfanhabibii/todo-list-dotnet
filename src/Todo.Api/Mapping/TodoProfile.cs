using AutoMapper;
using Todo.Api.Dtos;
using Todo.Core.Entities;

namespace Todo.Api.Mapping;

public class TodoProfile : Profile
{
    public TodoProfile()
    {
        // Read mappings
        CreateMap<TodoItem, TodoReadDto>();
        CreateMap<TodoItem, AdminTodoReadDto>();

        // Create mapping (server-side defaults)
        CreateMap<CreateTodoDto, TodoItem>()
            .ForMember(d => d.IsDone, opt => opt.MapFrom(_ => false))
            .ForMember(d => d.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Update (full replace)
        CreateMap<UpdateTodoDto, TodoItem>();

        // Patch (partial) - map only non-null props
        CreateMap<PatchTodoDto, TodoItem>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
