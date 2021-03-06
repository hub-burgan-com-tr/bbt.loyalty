import {Injectable} from '@angular/core';
import {Step} from "../models/step.model";
import {Subject, takeUntil} from "rxjs";
import {environment} from "../../environments/environment";
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiPaths} from "../models/api-paths";
import {ApiBaseResponseModel} from "../models/api-base-response.model";
import {GlobalVariable} from "../global";
import {UtilityService} from "./utility.service";
import {
  TargetDefinitionListRequestModel,
  TargetDefinitionAddRequestModel,
  TargetDefinitionUpdateRequestModel,
  TargetSourceAddUpdateRequestModel
} from "../models/target-definition";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';

@Injectable({
  providedIn: 'root'
})

export class TargetDefinitionService {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  stepData = [
    {id: 1, title: 'Hedef Tanımı', isActive: true, passed: false, route: 'definition'},
    {id: 2, title: 'Hedef Kaynağı', isActive: false, passed: false, route: 'source'},
  ];

  repostData = {
    id: null,
    listLink: GlobalVariable.targetList,
    previewButtonVisible: true,
    previewLink: GlobalVariable.targetPreview,
    copyModalMessage: 'Mevcut hedef tanımının aynısı olacak şekilde yeni bir hedef tanımı yapılacaktır. Onaylıyor musunuz?'
  };

  private baseUrl = environment.baseUrl;

  isTargetValuesChanged: boolean = false;

  constructor(private httpClient: HttpClient,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService) {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  getTargetViewTypes() {
    const url = `${this.baseUrl}/${ApiPaths.GetTargetViewTypes}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  getTargetSources() {
    const url = `${this.baseUrl}/${ApiPaths.GetTargetSources}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  targetDefinitionListGetByFilter(data: TargetDefinitionListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetDefinitionListGetByFilter}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  targetDefinitionListGetByFilterExcelFile(data: TargetDefinitionListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetDefinitionListGetExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  copyTarget(targetId: any) {
    let params = new HttpParams();
    params = params.append('targetId', targetId);

    const url = `${this.baseUrl}/${ApiPaths.CopyTarget}`;

    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params})
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.utilityService.redirectTo(`${GlobalVariable.targetUpdate}/${res.data.id}/definition`);
            this.toastrHandleService.success();
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  getTargetDetail(targetId: any) {
    const url = `${this.baseUrl}/${ApiPaths.GetTargetDetail}/${targetId}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  targetDefinitionAdd(data: TargetDefinitionAddRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetDefinitionAdd}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  targetDefinitionUpdate(data: TargetDefinitionUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetDefinitionUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  getTargetInfo(targetId: any) {
    let params = new HttpParams();
    params = params.append('id', targetId);

    const url = `${this.baseUrl}/${ApiPaths.TargetSourceViewForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  targetSourceGetInsertForm() {
    const url = `${this.baseUrl}/${ApiPaths.TargetSourceGetInsertForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  getTargetSource(targetId: any) {
    let params = new HttpParams();
    params = params.append('targetId', targetId);

    const url = `${this.baseUrl}/${ApiPaths.TargetSourceGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  targetSourceAdd(data: TargetSourceAddUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetSourceAdd}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  targetSourceUpdate(data: TargetSourceAddUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetSourceUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }
}
