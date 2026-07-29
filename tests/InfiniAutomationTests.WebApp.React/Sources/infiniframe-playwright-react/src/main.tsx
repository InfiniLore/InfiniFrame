import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import '../../../../InfiniAutomationTests.WebApp/src/style.css'
import App from './App'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <App/>
    </StrictMode>,
)
