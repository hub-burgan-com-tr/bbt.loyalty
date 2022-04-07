﻿using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CampainAchievementController : BaseController<CampainAchievementController>
    {
        private readonly ICampaignAchievementService _campaignAchievementService;

        public CampainAchievementController(ICampaignAchievementService campaignAchievementService)
        {
            _campaignAchievementService = campaignAchievementService;
        }
        /// <summary>
        /// Returns the campaign Achievement information by Id
        /// </summary>
        /// <param name="id">Campaign Achievement Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var adminSektor = await _campaignAchievementService.GetCampaignAchievementAsync(id);
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds new campaign Achievement
        /// </summary>
        /// <param name="campaignAchievement"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignAchievementInsertRequest campaignAchievement)
        {
            var createResult = await _campaignAchievementService.AddAsync(campaignAchievement);
            return Ok(createResult);
        }


        /// <summary>
        /// Updates campaign Achievement by Id
        /// </summary>
        /// <param name="campaignAchievement"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignAchievementInsertRequest campaignAchievement)
        {
            var result = await _campaignAchievementService.UpdateAsync(campaignAchievement);
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign Achievement by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _campaignAchievementService.DeleteAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign Achievement list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await _campaignAchievementService.GetListAsync();
            return Ok(result);
        }


        /// <summary>
        /// Returns the form data for insert page
        /// </summary>
        /// <param name="campaignId">Record Id of the capaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-insert-form")]
        public async Task<IActionResult> GetInsertForm(int campaignId)
        {
            var result = await _campaignAchievementService.GetInsertFormAsync(campaignId);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="campaignId">Record Id of the capaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int campaignId)
        {
            var result = await _campaignAchievementService.GetUpdateFormAsync(campaignId);
            return Ok(result);
        }


        ///// <summary>
        ///// Returns the campaign Achievement list by campaign Id
        ///// </summary>
        ///// <returns>returns as gropupped by channels</returns>
        //[HttpGet]
        //[Route("get-list-by-campaign")]
        //public async Task<IActionResult> GetListByCampaign(int campaignId)
        //{
        //    var result = await _campaignAchievementService.GetListByCampaignAsync(campaignId);
        //    return Ok(result);
        //}

    }
}