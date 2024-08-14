using AutoMapper;
using TasksApi.Models;
using TasksApi.Models.DTO;

namespace TasksApi.Profiles
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            CreateMap<AddAppUserDTO, AppUser>();
            CreateMap<AddTaskDTO, SingleTask>();
            CreateMap<EditTaskDTO, SingleTask>();
            CreateMap<AddClaimDTO, AppClaim>();
        }
    }
}
