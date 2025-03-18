using LifeKanbanApi.DTO;
using LifeKanbanApi.EndPoints.GetProject;
using LifeKanbanApi.Model;
using Mapster;

namespace LifeKanbanApi.Config;

  public static class MapsterConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Project, ProjectDto>.NewConfig()
                .Map(dest => dest.Tasks, src => src.Tasks)
                .Map(dest => dest.Milestones, src => src.Milestones);
                
            TypeAdapterConfig<ProjectTask, ProjectTaskDto>.NewConfig()
                .Map(dest => dest.ProjectId, src => src.ProjectId)
                .Map(dest => dest.Milestone, src => src.Milestone);
                    
            TypeAdapterConfig<Milestone, MilestoneDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.ProjectId, src => src.ProjectId);
        }
    }