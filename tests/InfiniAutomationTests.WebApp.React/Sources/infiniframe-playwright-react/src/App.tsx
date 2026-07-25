import CssBaseline from '@mui/material/CssBaseline'
import {createTheme, ThemeProvider} from '@mui/material/styles'
import WindowFeatureTestPanel from './WindowFeatureTestPanel'

const theme = createTheme({palette: {mode: 'dark'}})

export default function App() {
    return <ThemeProvider theme={theme}>
        <CssBaseline/>
        <WindowFeatureTestPanel/>
    </ThemeProvider>
}
