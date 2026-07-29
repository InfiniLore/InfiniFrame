import { Output, EventEmitter, Component, Input } from "@angular/core";

import { CommonModule } from "@angular/common";

export interface InputDataProbeProps {
  title: string;
  buttonText: string;
  data: string;
  onDataChanged: (value: string) => void;
  onSubmitted: (value: string) => void;
  titleId?: string;
  buttonId?: string;
  dataInputId?: string;
  dataLabel?: string;
}

@Component({
  selector: "input-data-probe",
  template: `
    <section class="data-probe">
      <h2 class="input-data-probe-title" [attr.id]="titleId">{{title}}</h2>
      <label class="data-probe-field"
        ><span>{{dataLabel || 'Input data'}}</span>
        <input
          [attr.id]="dataInputId"
          [value]="data"
          (input)="this.onDataChanged.emit($event.target.value)"
      /></label>
      <button [attr.id]="buttonId" (click)="this.onSubmitted.emit(data)">
        {{buttonText}}
      </button>
    </section>
  `,
  styles: [
    `
      :host {
        display: contents;
      }
    `,
  ],
  standalone: true,
  imports: [CommonModule],
})
export default class InputDataProbe {
  @Input() titleId!: InputDataProbeProps["titleId"];
  @Input() title!: InputDataProbeProps["title"];
  @Input() dataLabel!: InputDataProbeProps["dataLabel"];
  @Input() dataInputId!: InputDataProbeProps["dataInputId"];
  @Input() data!: InputDataProbeProps["data"];
  @Input() buttonId!: InputDataProbeProps["buttonId"];
  @Input() buttonText!: InputDataProbeProps["buttonText"];
  @Output("dataChanged") onDataChanged = new EventEmitter<any>();
  @Output("submitted") onSubmitted = new EventEmitter<any>();
}
