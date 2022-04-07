import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {UtilityService} from "../../../services/utility.service";
import {ListService} from "../../../services/list.service";
import {CampaignLimitsService} from "../../../services/campaign-limits.service";
import { CampaignLimitsListRequestModel } from 'src/app/models/campaign-limits';
import {saveAs} from 'file-saver';
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-campaign-limits-list',
  templateUrl: './campaign-limits-list.component.html',
  styleUrls: ['./campaign-limits-list.component.scss']
})

export class CampaignLimitsListComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  columns = [
    {columnName: 'Çatı Limiti Adı', propertyName: 'name', isBoolean: false},
    {columnName: 'Kazanım Sıklığı', propertyName: 'achievementFrequency', isBoolean: false},
    {columnName: 'Para Birimi', propertyName: 'currency', isBoolean: false},
    {columnName: 'Çatı Max Tutar', propertyName: 'maxTopLimitAmount', isBoolean: false},
    {columnName: 'Çatı Max Yararlanma', propertyName: 'maxTopLimitUtilization', isBoolean: false},
    {columnName: 'Çatı Oranı', propertyName: 'maxTopLimitRate', isBoolean: false},
    {columnName: 'Tutar', propertyName: 'amount', isBoolean: true},
    {columnName: 'Oran', propertyName: 'rate', isBoolean: true},
    {columnName: 'Aktif', propertyName: 'isActive', isBoolean: true}
  ];

  achievementFrequencyList: DropdownListModel[];
  currencyTypeList: DropdownListModel[];
  typeList: DropdownListModel[];

  filterForm = {
    name: '',
    achievementFrequencyId: null,
    currencyId: null,
    maxTopLimitAmount: '',
    maxTopLimitUtilization: '',
    maxTopLimitRate: '',
    type: null,
    isActive: null
  };

  constructor(private campaignLimitsService: CampaignLimitsService,
              private toastrService: ToastrService,
              private utilityService: UtilityService,
              private listService: ListService) {
  }

  ngOnInit(): void {
    this.getParameterList();
    this.campaignLimitsListGetByFilter();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  clear() {
    this.filterForm = {
      name: '',
      achievementFrequencyId: null,
      currencyId: null,
      maxTopLimitAmount: '',
      maxTopLimitUtilization: '',
      maxTopLimitRate: '',
      type: null,
      isActive: null
    };

    this.listService.clearList();

    this.campaignLimitsListGetByFilter();
  }

  getParameterList() {
    this.campaignLimitsService.getParameterList()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.achievementFrequencyList = res.data.achievementFrequencyList;
            this.currencyTypeList = res.data.currencyList;
            this.typeList = res.data.typeList;
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error.hasError)
            this.toastrService.error(err.error.errorMessage);
        }
      });
  }

  campaignLimitsListGetByFilter() {
    let requestModel: CampaignLimitsListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      name: this.filterForm.name,
      achievementFrequencyId: this.filterForm.achievementFrequencyId,
      currencyId: this.filterForm.currencyId,
      maxTopLimitAmount: parseInt(this.filterForm.maxTopLimitAmount),
      maxTopLimitUtilization: parseInt(this.filterForm.maxTopLimitUtilization),
      maxTopLimitRate: parseInt(this.filterForm.maxTopLimitRate),
      type: this.filterForm.type,
      isActive: this.filterForm.isActive
    };
    this.campaignLimitsService.campaignLimitsListGetByFilter(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data && res.data.responseList.length > 0) {
            this.listService.setList(this.columns, this.setRouterLinks(res.data.responseList), res.data.paging);
          } else {
            this.listService.setError("Listeleme için uygun kayıt bulunamadı");
          }
        },
        error: err => {
          if (err.error.hasError) {
            this.listService.setError(err.error.errorMessage);
          }
        }
      });
  }

  campaignLimitsListGetByFilterExcelFile() {
    let requestModel: CampaignLimitsListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      name: this.filterForm.name,
      achievementFrequencyId: this.filterForm.achievementFrequencyId,
      currencyId: this.filterForm.currencyId,
      maxTopLimitAmount: parseInt(this.filterForm.maxTopLimitAmount),
      maxTopLimitUtilization: parseInt(this.filterForm.maxTopLimitUtilization),
      maxTopLimitRate: parseInt(this.filterForm.maxTopLimitRate),
      type: this.filterForm.type,
      isActive: this.filterForm.isActive
    };
    this.campaignLimitsService.campaignLimitsListGetByFilterExcelFile(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data?.document) {
            let document = res.data.document;
            let file = this.utilityService.convertBase64ToFile(document.data, document.documentName, document.mimeType);
            saveAs(file, res.data?.document.documentName);
            this.toastrService.success("İşlem başarılı");
          } else {
            this.toastrService.error(res.errorMessage);
          }
        },
        error: err => {
          if (err.error.hasError) {
            this.toastrService.error(err.error.errorMessage);
          }
        }
      });
  }

  setRouterLinks(responseList) {
    responseList.map(res => {
      res.routerLink = `../update/${res.id}/limit`;
    });
    return responseList;
  }
}