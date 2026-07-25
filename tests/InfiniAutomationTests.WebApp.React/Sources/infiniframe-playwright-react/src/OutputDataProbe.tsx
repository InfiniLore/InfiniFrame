import Button from '@mui/material/Button'
import Paper from '@mui/material/Paper'
import Stack from '@mui/material/Stack'
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'

type OutputDataProbeProps = {
    title: string
    buttonText: string
    data?: string
    onReadRequested: () => void | Promise<void>
    titleId?: string
    buttonId?: string
    dataInputId?: string
    dataLabel?: string
}

export default function OutputDataProbe({
                                            title,
                                            buttonText,
                                            data,
                                            onReadRequested,
                                            titleId,
                                            buttonId,
                                            dataInputId,
                                            dataLabel = 'Output data'
                                        }: OutputDataProbeProps) {
    return <Paper variant="outlined" sx={{p: 2}}>
        <Stack spacing={2}>
            <Typography id={titleId} className="output-data-probe-title" variant="h6">{title}</Typography>
            <TextField id={dataInputId} label={dataLabel} value={data ?? ''} slotProps={{htmlInput: {readOnly: true}}}/>
            <Button id={buttonId} variant="contained" onClick={onReadRequested}>{buttonText}</Button>
        </Stack>
    </Paper>
}
