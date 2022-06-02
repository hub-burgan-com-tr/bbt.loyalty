export enum ApiPaths {
  Login = 'Authorization/login',

  GetProgramTypes = 'Parameter/get-program-types',
  GetCampaignDetail = 'Campaign/get',
  CopyCampaign = 'Approve/copy-campaign',
  GetCampaignInfo = 'Customer/view-customer-min',
  CampaignDefinitionList = 'Campaign/get-by-filter',
  CampaignDefinitionListGetExcelFile = 'Campaign/get-by-filter-excel',
  CampaignDefinitionAdd = 'Campaign/add',
  CampaignDefinitionUpdate = 'Campaign/update',
  CampaignDefinitionGetInsertForm = 'Campaign/get-insert-form',
  CampaignDefinitionGetUpdateForm = 'Campaign/get-update-form',
  CampaignDefinitionGetContractFile = 'Campaign/get-contract-file',
  CampaignRulesAdd = 'CampaignRule/add',
  CampaignRulesUpdate = 'CampaignRule/update',
  CampaignRulesGetInsertForm = 'CampaignRule/get-insert-form',
  CampaignRulesGetUpdateForm = 'CampaignRule/get-update-form',
  CampaignRulesGetIdentityFile = 'CampaignRule/get-identity-file',
  CampaignTargetsAdd = 'CampaignTarget/add',
  CampaignTargetsUpdate = 'CampaignTarget/update',
  CampaignTargetsGetUpdateForm = 'CampaignTarget/get-update-form',
  CampaignTargetsGetInsertForm = 'CampaignTarget/get-insert-form',
  CampaignGainChannelsGetUpdateForm = 'CampaignChannelCode/get-update-form',
  CampaignGainChannelsUpdate = 'CampaignChannelCode/update',
  CampaignGainsGetUpdateForm = 'CampainAchievement/get-update-form',
  CampaignGainsUpdate = 'CampainAchievement/update',

  CampaignLimitsGetParameterList = 'CampaignTopLimit/get-parameter-list',
  CampaignLimitsGetByFilter = 'CampaignTopLimit/get-by-filter',
  CampaignLimitsListGetExcelFile = 'CampaignTopLimit/get-by-filter-excel',
  CampaignLimitGetInsertForm = 'CampaignTopLimit/get-insert-form',
  GetLimitDetail = 'CampaignTopLimit/get-update-form',
  CopyLimit = 'Approve/copy-top-limit',
  CampaignLimitAdd = 'CampaignTopLimit/add',
  CampaignLimitUpdate = 'CampaignTopLimit/update',

  GetTargetViewTypes = 'Parameter/get-target-views',
  GetTargetSources = 'Parameter/get-target-sources',
  TargetDefinitionListGetByFilter = 'Target/get-by-filter',
  TargetDefinitionListGetExcelFile = 'Target/get-list-excel',
  TargetSourceViewForm = 'Target/get-view-form',
  CopyTarget = 'Approve/copy-target',
  GetTargetDetail = 'Target/get',
  TargetDefinitionAdd = 'Target/add',
  TargetDefinitionUpdate = 'Target/update',
  TargetSourceGetInsertForm = 'TargetDetail/get-insert-form',
  TargetSourceGetUpdateForm = 'TargetDetail/get-update-form',
  TargetSourceAdd = 'TargetDetail/add',
  TargetSourceUpdate = 'TargetDetail/update',

  CampaignReportFilterForm = 'Report/get-campaign-report-form',
  CampaignReportGetByFilter = 'Report/get-campaignreport-by-filter',
  CampaignReportGetByFilterExcelFile = 'Report/get-campaign-report-by-filter-excel',
  CustomerReportFilterForm = 'Report/get-customer-report-form',
  CustomerReportGetByFilter = 'Report/get-customer-report-by-filter',
  CustomerReportGetByFilterExcelFile = 'Report/get-customer-report-by-filter-excel',
  CustomerDetail = 'Report/get-customer-report-detail',

  CampaignDefinitionApproveForm = 'Approve/get-campaign-form',
  CampaignRuleDocumentDownload = 'Approve/document-download',
  CampaignDefinitionApproveState = 'Approve/campaign',

  CampaignLimitsApproveForm = 'Approve/get-toplimit-approval-form',
  CampaignLimitsApproveState = 'Approve/top-limit',

  TargetDefinitionApproveForm = 'Approve/get-target-approval-form',
  TargetDefinitionApproveState = 'Approve/target-definition',
}
