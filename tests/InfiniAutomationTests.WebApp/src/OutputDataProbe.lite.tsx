import { useMetadata } from '@builder.io/mitosis'

useMetadata({ angular: { selector: 'output-data-probe', nativeAttributes: ['value', 'readOnly'] } })

export interface OutputDataProbeProps {
  title: string
  buttonText: string
  data: string
  onReadRequested: () => void
  titleId?: string
  buttonId?: string
  dataInputId?: string
  dataLabel?: string
}

export default function OutputDataProbe(props: OutputDataProbeProps) {
  return <section class="data-probe">
    <h2 id={props.titleId} class="output-data-probe-title">{props.title}</h2>
    <label class="data-probe-field">
      <span>{props.dataLabel || 'Output data'}</span>
      <input id={props.dataInputId} value={props.data} readOnly={true}/>
    </label>
    <button id={props.buttonId} onClick={() => props.onReadRequested()}>{props.buttonText}</button>
  </section>
}
