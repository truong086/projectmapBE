﻿using projectmap.Common;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public interface IRepairDetailsService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllNoDoneByAdmin(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllDoneByAdmin(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllDoneByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllNoDoneByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<RepairDetailsDTO>> Add(RepairDetailsDTO data);
        Task<PayLoad<RepairDetailsUpdate>> Update(RepairDetailsUpdate data);
        Task<PayLoad<RepairDetailsUpdateByAccont>> UpdateByAccout(RepairDetailsUpdateByAccont data);
        Task<PayLoad<object>> FindOneId(int id);
    }
}
