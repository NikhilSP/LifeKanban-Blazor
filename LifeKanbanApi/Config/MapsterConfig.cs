using LifeKanbanApi.DTO;
using LifeKanbanApi.Model;
using Mapster;

namespace LifeKanbanApi.Config;

  public static class MapsterConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Project, ProjectDto>.NewConfig()
                .Map(dest => dest.Tasks, src => src.Tasks)
                .Map(dest => dest.Milestones, src => src.Milestones)
                .Map(dest => dest.Position, src => src.Position)
                .Map(dest => dest.Description, src => src.Description);
            
            TypeAdapterConfig<ProjectTask, ProjectTaskDto>.NewConfig()
                .Map(dest => dest.ProjectId, src => src.ProjectId)
                .Map(dest => dest.Milestone, src => src.Milestone)
                .Map(dest => dest.SubTasks, src => src.SubTasks);
                
            TypeAdapterConfig<Milestone, MilestoneDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.ProjectId, src => src.ProjectId);
            
            TypeAdapterConfig<SubTask, SubTaskDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.IsCompleted, src => src.IsCompleted)
                .Map(dest => dest.Position, src => src.Position);
            
            TypeAdapterConfig<QuickTodo, QuickTodoDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.IsCompleted, src => src.IsCompleted)
                .Map(dest => dest.DateCreated, src => src.DateCreated)
                .Map(dest => dest.DateCompleted, src => src.DateCompleted);
        }
    }