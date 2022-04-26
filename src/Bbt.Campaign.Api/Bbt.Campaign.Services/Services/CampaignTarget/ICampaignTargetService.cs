﻿using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Models.CampaignTarget;

namespace Bbt.Campaign.Services.Services.CampaignTarget
{
    public interface ICampaignTargetService
    {
        public Task<BaseResponse<CampaignTargetDto>> GetCampaignTargetAsync(int id);
        public Task<BaseResponse<CampaignTargetDto>> UpdateAsync(CampaignTargetInsertRequest request);
        public Task<BaseResponse<List<CampaignTargetDto>>> GetListAsync(); 
        public Task<BaseResponse<CampaignTargetDto>> GetListByCampaignAsync(int campaignId);
        public Task<BaseResponse<CampaignTargetDto>> DeleteAsync(int id);
        public Task<BaseResponse<CampaignTargetInsertFormDto>> GetInsertForm();
        public Task<BaseResponse<CampaignTargetUpdateFormDto>> GetUpdateForm(int campaignId);
        public Task<CampaignTargetDto> GetCampaignTargetDto(int campaignId, bool isRemoveInvisible);
        public Task<CampaignTargetDto> GetCampaignTargetDtoCustomer(int campaignId, List<TargetParameterDto> targetSourceList);
        public Task<CampaignTargetDto> GetCampaignTargetDtoTestCustomer(int campaignId);
    }
}
