using GroupsCourseService.Dtos;
using GroupsCourseService.Model;

namespace GroupsCourseService.Service
{
    public  interface  IGroupService
    {
        Task<GeneralResponse<ResponseGroupDto>> CreateNewGroup(RequestCreateNewGroup dto);

       Task<List<ResponseGroupDto>> GetAllAsync();

        Task<GeneralResponse<ResponseGroupDto>> GetByIdAsync(Guid id);

        Task<GeneralResponse<ResponseGroupDto>> CompleteGroupAsync(Guid id);

        Task<GeneralResponse<ResponseGroupDto>> UpdateStatusGroupAsync(Guid id, UpdateGroupStatusDto statusDto);
        Task<GeneralResponse<ResponseGroupDto>> UpdateGroupAssignmentAsync(Guid id, UpdateGroupAssignmentDto dto);
    }
}
