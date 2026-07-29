import { useMetadata } from '@builder.io/mitosis'

useMetadata({ angular: { selector: 'input-data-probe', nativeAttributes: ['value'] } })

export interface InputDataProbeProps {
  title: string
  buttonText: string
  data: string
  onDataChanged: (value: string) => void
  onSubmitted: (value: string) => void
  titleId?: string
  buttonId?: string
  dataInputId?: string
  dataLabel?: string
}

export default function InputDataProbe(props: InputDataProbeProps) {
  return <section class="data-probe">
    <h2 id={props.titleId} class="input-data-probe-title">{props.title}</h2>
    <label class="data-probe-field">
      <span>{props.dataLabel || 'Input data'}</span>
      <input
        id={props.dataInputId}
        value={props.data}
        onInput={(event) => props.onDataChanged(event.target.value)}
      />
    </label>
    <button id={props.buttonId} onClick={() => props.onSubmitted(props.data)}>{props.buttonText}</button>
  </section>
}
