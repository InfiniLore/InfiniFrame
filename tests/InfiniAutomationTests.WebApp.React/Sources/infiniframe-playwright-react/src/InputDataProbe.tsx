import Button from '@mui/material/Button'
import Paper from '@mui/material/Paper'
import Stack from '@mui/material/Stack'
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'

type InputDataProbeProps = {
    title: string
    buttonText: string
    data: string
    onDataChanged: (value: string) => void
    onSubmitted: (value: string) => void | Promise<void>
    titleId?: string
    buttonId?: string
    dataInputId?: string
    dataLabel?: string
}

export default function InputDataProbe({
    title,
    buttonText,
    data,
    onDataChanged,
    onSubmitted,
    titleId,
    buttonId,
    dataInputId,
    dataLabel = 'Input data'
}: InputDataProbeProps) {
    return <Paper variant="outlined" sx={{p: 2}}>
        <Stack spacing={2}>
            <Typography id={titleId} className="input-data-probe-title" variant="h6">{title}</Typography>
            <TextField
                id={dataInputId}
                label={dataLabel}
                value={data}
                onChange={event => onDataChanged(event.target.value)}
            />
            <Button id={buttonId} variant="contained" onClick={() => onSubmitted(data)}>{buttonText}</Button>
        </Stack>
    </Paper>
}
