import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {ApiPaths} from '../models/api-paths';
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiBaseResponseModel} from '../models/api-base-response.model';

@Injectable({
  providedIn: 'root'
})

export class ApproveService {
  private baseUrl = environment.baseUrl;

  constructor(private httpClient: HttpClient) {
  }

  campaignDefinitionApproveForm(id: any) {
    let params = new HttpParams();
    params = params.append('id', id);

    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionApproveForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignDefinitionApprove(id: any) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionApprove}/${id}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignDefinitionDisapprove(id: any) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionDisapprove}/${id}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignLimitsApproveForm(id: any) {
    let params = new HttpParams();
    params = params.append('id', id);

    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitsApproveForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignLimitsApproveState(id: any, state: boolean) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitsApproveState}/${id}/${state}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, {});
  }

  targetDefinitionApproveForm(id: any) {
    let params = new HttpParams();
    params = params.append('id', id);

    const url = `${this.baseUrl}/${ApiPaths.TargetDefinitionApproveForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  targetDefinitionApproveState(id: any, state: boolean) {
    const url = `${this.baseUrl}/${ApiPaths.TargetDefinitionApproveState}/${id}/${state}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, {});
  }
}
