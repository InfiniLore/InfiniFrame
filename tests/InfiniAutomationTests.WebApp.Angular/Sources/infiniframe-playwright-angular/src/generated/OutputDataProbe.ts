import { Output, EventEmitter, Component, Input } from "@angular/core";

import { CommonModule } from "@angular/common";

export interface OutputDataProbeProps {
  title: string;
  buttonText: string;
  data: string;
  onReadRequested: () => void;
  titleId?: string;
  buttonId?: string;
  dataInputId?: string;
  dataLabel?: string;
}

@Component({
  selector: "output-data-probe",
  template: `
    <section class="data-probe">
      <h2 class="output-data-probe-title" [attr.id]="titleId">{{title}}</h2>
      <label class="data-probe-field"
        ><span>{{dataLabel || 'Output data'}}</span>
        <input [attr.id]="dataInputId" [value]="data" [readOnly]="true"
      /></label>
      <button [attr.id]="buttonId" (click)="this.onReadRequested.emit()">
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
export default class OutputDataProbe {
  @Input() titleId!: OutputDataProbeProps["titleId"];
  @Input() title!: OutputDataProbeProps["title"];
  @Input() dataLabel!: OutputDataProbeProps["dataLabel"];
  @Input() dataInputId!: OutputDataProbeProps["dataInputId"];
  @Input() data!: OutputDataProbeProps["data"];
  @Input() buttonId!: OutputDataProbeProps["buttonId"];
  @Input() buttonText!: OutputDataProbeProps["buttonText"];
  @Output("readRequested") onReadRequested = new EventEmitter<any>();
}
