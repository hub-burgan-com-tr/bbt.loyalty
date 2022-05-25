import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss']
})
export class MainContentComponent implements OnInit {
  @Input('blockTitle') blockTitle = '';
  @Input('route') route = '';
  @Input('newButtonText') newButtonText = '';
  @Input('createAuthorization') createAuthorization: boolean = false;
  @Input('isReportsPage') isReportsPage: boolean = false;

  constructor() {
  }

  ngOnInit(): void {
  }

}
