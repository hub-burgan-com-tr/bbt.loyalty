using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Customer;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Bbt.Campaign.Services.Services.Remote;

namespace Bbt.Campaign.Services.Services.Customer
{
    public class CustomerService : ICustomerService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;
        private readonly ICampaignRuleService _campaignRuleService;
        private readonly ICampaignTargetService _campaignTargetService;
        private readonly ICampaignAchievementService _campaignAchievementService;
        private readonly IRemoteService _remoteService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService,
            ICampaignService campaignService, ICampaignRuleService campaignRuleService, ICampaignTargetService campaignTargetService,
            ICampaignAchievementService campaignAchievementService, IRemoteService remoteService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
            _campaignRuleService = campaignRuleService;
            _campaignTargetService = campaignTargetService;
            _campaignAchievementService = campaignAchievementService;
            _remoteService = remoteService;
        }

        public async Task<BaseResponse<CustomerCampaignDto>> SetJoin(SetJoinRequest request) 
        {
            await CheckValidationAsync(request.CustomerCode, request.CampaignId);

            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && !x.IsDeleted)
               .FirstOrDefaultAsync();
            if (entity != null)
            {
                if(entity.IsJoin)
                    throw new Exception("Müşteri bu kampayaya daha önceki bir tarihte katılmış.");

                entity.IsJoin = request.IsJoin;
                entity.StartDate = request.IsJoin ? DateTime.Now : null;
                entity.LastModifiedBy = request.CustomerCode;

                await _unitOfWork.GetRepository<CustomerCampaignEntity>().UpdateAsync(entity);
            }
            else
            {
                entity = new CustomerCampaignEntity();
                entity.CustomerCode = request.CustomerCode;
                entity.CampaignId = request.CampaignId;
                entity.IsFavorite = false;
                entity.IsJoin = request.IsJoin;
                entity.StartDate = request.IsJoin ? Helpers.ConvertDateTimeToShortDate(DateTime.Now) : null;
                entity.CreatedBy = request.CustomerCode;

                entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(entity);
            }

            await _unitOfWork.SaveChangesAsync();

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        public async Task<BaseResponse<CustomerCampaignDto>> SetFavorite(SetFavoriteRequest request)
        {
            await CheckValidationAsync(request.CustomerCode, request.CampaignId);

            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && x.IsDeleted != true)
               .FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.IsFavorite = request.IsFavorite;
                entity.LastModifiedBy = request.CustomerCode;

                await _unitOfWork.GetRepository<CustomerCampaignEntity>().UpdateAsync(entity);
            }
            else
            {
                entity = new CustomerCampaignEntity();
                entity.CustomerCode = request.CustomerCode;
                entity.CampaignId = request.CampaignId;
                entity.IsFavorite = request.IsFavorite; 
                entity.IsJoin = false;
                entity.CreatedBy = request.CustomerCode;

                entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(entity);
            }

            await _unitOfWork.SaveChangesAsync();

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        private async Task CheckValidationAsync(string customerCode, int campaignId) 
        {
            if(string.IsNullOrEmpty(customerCode))
                throw new Exception("Müşteri kodu giriniz.");

            if(campaignId <= 0)
                throw new Exception("Kampanya giriniz.");

            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
            var campaignEntity = _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == campaignId && !x.IsDeleted && x.IsActive && x.EndDate >= today)
                    .FirstOrDefault();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }
        }
        public async Task<BaseResponse<CustomerCampaignDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.GetRepository<CustomerCampaignEntity>().DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetCustomerCampaignAsync(entity.Id);
            }
            return await BaseResponse<CustomerCampaignDto>.FailAsync("Kayıt bulunamadı.");
        }
        public async Task<BaseResponse<CustomerCampaignDto>> GetCustomerCampaignAsync(int id)
        {
            var customerCampaignEntity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().GetByIdAsync(id);
            if (customerCampaignEntity != null)
            {
                CustomerCampaignDto customerCampaignDto = _mapper.Map<CustomerCampaignDto>(customerCampaignEntity);
                return await BaseResponse<CustomerCampaignDto>.SuccessAsync(customerCampaignDto);
            }
            return null;
        }
        public async Task<BaseResponse<CustomerCampaignListFilterResponse>> GetByFilterAsync(CustomerCampaignListFilterRequest request)
        {
            if(request.PageTypeId == (int)CustomerCampaignListTypeEnum.Join || request.PageTypeId == (int)CustomerCampaignListTypeEnum.Favorite) 
            {
                if (string.IsNullOrEmpty(request.CustomerCode))
                    throw new Exception("Müşteri kodu giriniz.");
            }

            CustomerCampaignListFilterResponse response = new CustomerCampaignListFilterResponse();
            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);

            IQueryable<CampaignDetailListEntity> campaignQuery = await GetCampaignQueryAsync(request);
            if (campaignQuery.Count() == 0)
                return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response, "Kampanya bulunamadı");

            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = campaignQuery.Count();
            campaignQuery = campaignQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);
           
            var campaignList = campaignQuery.Select(x => new CampaignMinDto
            {
                Id = x.Id,
                Name = x.Name,
                TitleEn = x.TitleEn,
                TitleTr = x.TitleTr,
                CampaignListImageUrl = x.CampaignListImageUrl,
                CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                EndDate = x.EndDate,
            }).ToList();

            var customerCampaignList = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                .GetAll(x => !x.IsDeleted && x.CustomerCode == (request.CustomerCode ?? string.Empty))
                .ToListAsync();
                
            List<CustomerCampaignMinListDto> returnList = new List<CustomerCampaignMinListDto>();
            foreach (var campaign in campaignList)
            {
                CustomerCampaignMinListDto customerCampaignListDto = new CustomerCampaignMinListDto();

                customerCampaignListDto.CampaignId = campaign.Id;
                customerCampaignListDto.Campaign = campaign;
                customerCampaignListDto.CustomerCode = request.CustomerCode;
                customerCampaignListDto.IsJoin = false;
                customerCampaignListDto.IsFavorite = false;

                var customerCampaign = customerCampaignList.Where(x => x.CampaignId == campaign.Id).FirstOrDefault();
                if (customerCampaign != null)
                {
                    customerCampaignListDto.Id = customerCampaign.Id;
                    customerCampaignListDto.IsJoin = customerCampaign.IsJoin;
                    customerCampaignListDto.IsFavorite = customerCampaign.IsFavorite;
                }

                if (campaign.EndDate > today)
                {
                    TimeSpan ts = campaign.EndDate - today;
                    customerCampaignListDto.DueDay = ts.Days + 1;
                }

                if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Campaign)
                {
                    if (campaign.EndDate >= today)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Join)
                {
                    if (customerCampaignListDto.IsJoin)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Favorite)
                {
                    if (customerCampaignListDto.IsFavorite)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.OverDue)
                {
                    if (DateTime.UtcNow > campaign.EndDate)
                        returnList.Add(customerCampaignListDto);
                }
            }

            response.CustomerCampaignList = returnList;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            
            return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response);
        }
        private async Task<IQueryable<CampaignDetailListEntity>> GetCampaignQueryAsync(CustomerCampaignListFilterRequest request) 
        {
            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
            var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                .GetAll(x => !x.IsDeleted && x.IsActive 
                        && (x.ViewOptionId != (int)ViewOptionsEnum.InvisibleCampaign || x.ViewOptionId == null)
                        && x.StatusId == (int)StatusEnum.Approved);

            if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Campaign)
                campaignQuery = campaignQuery.Where(t => t.EndDate >= today);
            else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.OverDue)
                campaignQuery = campaignQuery.Where(t => t.EndDate < today);

            //sort

            if (string.IsNullOrEmpty(request.SortBy))
            {
                campaignQuery = campaignQuery.OrderByDescending(x => x.Id);
                return campaignQuery;
            }

            if (request.SortBy.EndsWith("Str"))
                request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

            bool isDescending = request.SortDir?.ToLower() == "desc";

            if (request.SortBy.Equals("Id") || request.SortBy.Equals("Code")) 
            { 
                campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Id) : campaignQuery = campaignQuery.OrderBy(x => x.Id);
            }
            else if (request.SortBy.Equals("Order"))
            {
                campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Order) : campaignQuery = campaignQuery.OrderBy(x => x.Order);
            }

            return campaignQuery;
        }
        public async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerViewFormAsync(int campaignId, string contentRootPath)
        {
            CustomerViewFormMinDto response = new CustomerViewFormMinDto();

            //campaign
            response.CampaignId = campaignId;

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
            }

            response.IsInvisibleCampaign = false;

            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
            }

            response.IsJoin = false;

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            response.IsContract = false;
            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0)
            {
                response.IsContract = false;
                response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);
            }

            //target

            //var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDto(campaignId, true);

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer2(campaignId, string.Empty, "tr", true);

            response.CampaignTarget = campaignTargetDto;

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerJoinFormDto>> GetCustomerJoinFormAsync(int campaignId, string customerCode, string contentRootPath)
        {
            if (string.IsNullOrEmpty(customerCode))
                throw new Exception("Müşteri kodu giriniz.");

            CustomerJoinFormDto response = new CustomerJoinFormDto();

            //campaign
            response.CampaignId = campaignId;

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
            }

            int viewOptionId = campaignEntity.ViewOptionId ?? 0;
            response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;

            response.IsOverDue = DateTime.Now.AddDays(-1) > campaignEntity.EndDate;

            response.IsJoin = false;
            var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && !x.IsDeleted)
                        .FirstOrDefaultAsync();
            if (customerJoin != null)
            {
                response.IsJoin = customerJoin.IsJoin;
            }

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            response.IsContract = false;
            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0) 
            {
                response.IsContract = false;
                response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);
            }
                
            //target

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer(campaignId, 0, 0);

            response.CampaignTarget = campaignTargetDto;

            //achievement
            //var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            //response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerJoinFormDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerAchievementFormDto>> GetCustomerAchievementFormAsync(int campaignId, string customerCode, string? language)
        {
            CustomerAchievementFormDto response = new CustomerAchievementFormDto();

            if (language == null)
                language = "tr";

            //campaign
            response.CampaignId = campaignId;
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }

            response.IsInvisibleCampaign = false;
            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
            }
            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);
            response.Campaign = campaignDto;

            decimal? totalAchievement = 0;
            decimal? previousMonthAchievement = 0;
            //decimal usedAmount = 0;
            //int usedNumberOfTransaction = 0;


            response.IsAchieved = false;
            string serviceUrl = string.Empty;

            if (StaticValues.IsDevelopment) 
            {
                totalAchievement = 190;
                previousMonthAchievement = 120;
                response.TotalAchievementStr = Helpers.ConvertNullablePriceString(totalAchievement);
                response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(previousMonthAchievement);
                response.TotalAchievementCurrencyCode = "TRY";
                response.PreviousMonthAchievementCurrencyCode = "TRY";
            }
            else 
            {
                var goalResultByCustomerIdAndMonthCount = await _remoteService.GetGoalResultByCustomerIdAndMonthCountData(customerCode);
                if (goalResultByCustomerIdAndMonthCount != null)
                {
                    if (goalResultByCustomerIdAndMonthCount.Total != null)
                    {
                        response.TotalAchievementStr = Helpers.ConvertNullablePriceString(goalResultByCustomerIdAndMonthCount.Total.Amount);
                        response.TotalAchievementCurrencyCode = goalResultByCustomerIdAndMonthCount.Total.Currency;
                    }
                    if (goalResultByCustomerIdAndMonthCount.Months != null && goalResultByCustomerIdAndMonthCount.Months.Any())
                    {
                        int month = DateTime.Now.Month;
                        int year = DateTime.Now.Year;

                        if (month == 1)
                        {
                            month = 12;
                            year = year - 1;
                        }
                        else
                        {
                            month = month - 1;
                        }

                        var monthAchievent = goalResultByCustomerIdAndMonthCount.Months.Where(x => x.Year == year && x.Month == month).FirstOrDefault();
                        if (monthAchievent != null)
                        {
                            response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(monthAchievent.Amount);
                            response.PreviousMonthAchievementCurrencyCode = monthAchievent.Currency;
                        }
                    }
                }
            }

            response.CampaignTarget =  await _campaignTargetService.GetCampaignTargetDtoCustomer2(campaignId, customerCode, language, false);

            response.IsAchieved = response.CampaignTarget.IsAchieved;

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCustomerAchievementsAsync(campaignId, customerCode, language);
            foreach (var campaignAchievement in campaignAchievementList)
                campaignAchievement.IsAchieved = response.IsAchieved;
            response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerAchievementFormDto>.SuccessAsync(response);
        }


        public async Task<BaseResponse<CustomerJoinSuccessFormDto>> GetCustomerJoinSuccessFormAsync(int campaignId, string customerCode) 
        {
            CustomerJoinSuccessFormDto response = new CustomerJoinSuccessFormDto();

            var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && !x.IsDeleted)
                        .FirstOrDefaultAsync();
            if (customerJoin == null)
                throw new Exception("Müşteri kampanyaya katılmamış.");

            if (!customerJoin.IsJoin)
                throw new Exception("Müşteri kampanyaya katılmamış.");

            var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted);
            campaignQuery = campaignQuery.Take(1);

            var campaignList = campaignQuery.Select(x => new CampaignMinDto
            {
                Id = x.Id,
                Name = x.Name,
                TitleEn = x.TitleEn,
                TitleTr = x.TitleTr,
                CampaignListImageUrl = x.CampaignListImageUrl,
                CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                EndDate = x.EndDate,
            }).ToList();

            if(!campaignList.Any())
                throw new Exception("Kampanya bulunamadı.");

            response.Campaign = campaignList[0];

            return await BaseResponse<CustomerJoinSuccessFormDto>.SuccessAsync(response);
        }
    }
}
