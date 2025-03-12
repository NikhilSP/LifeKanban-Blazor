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
                .Map(dest => dest.MilestoneId, src => src.MilestoneId);
                    
            TypeAdapterConfig<Milestone, MilestoneDto>.NewConfig()
                .Map(dest => dest.ProjectId, src => src.ProjectId);
            
        }
    }