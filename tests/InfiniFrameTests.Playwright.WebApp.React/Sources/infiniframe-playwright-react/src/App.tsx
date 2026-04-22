import { useState } from 'react'

function App() {
    const [isFullscreen, setIsFullscreen] = useState(false)
    const [isToggledToNewTitle, setIsToggledToNewTitle] = useState(false)
    const [oldTitle, setOldTitle] = useState('')

    const toggleFullscreen = async () => {
        try {
            if (!document.fullscreenElement) {
                await document.body.requestFullscreen()
                setIsFullscreen(true)
            } else if (document.exitFullscreen) {
                await document.exitFullscreen()
                setIsFullscreen(false)
            }
        } catch (error) {
            console.error('Failed to toggle fullscreen', error)
        }
    }

    const toggleTitle = () => {
        if (!isToggledToNewTitle) {
            setOldTitle(document.title)
            document.title = 'New Title'
            setIsToggledToNewTitle(true)
            return
        }

        document.title = oldTitle
        setOldTitle('')
        setIsToggledToNewTitle(false)
    }

    return (
        <>
            <button id="fullscreen-toggle-button" onClick={toggleFullscreen}>
                {isFullscreen ? 'Exit Fullscreen' : 'Enter Fullscreen'}
            </button>
            <button id="title-toggle-button" onClick={toggleTitle}>
                {isToggledToNewTitle ? 'Reset Title' : 'Define New Title'}
            </button>
        </>
    )
}

export default App
