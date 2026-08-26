const { useState } = React;

function App() {
    const [count, setCount] = useState(0);

    return (
        <div className="app">
            <h1>InfiniFrame + React</h1>
            <p className="subtitle">Single-file executable with embedded React app</p>
            <div className="card">
                <button onClick={() => setCount(c => c + 1)}>
                    Count: {count}
                </button>
            </div>
            <p className="info">
                This React app runs from an embedded resource inside a single .exe file.
            </p>
        </div>
    );
}

ReactDOM.createRoot(document.getElementById('root')).render(<App />);
