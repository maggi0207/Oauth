import React, { useState } from "react";

const App: React.FC = () => {
  const [rollDay, setRollDay] = useState(10);
  const [count, setCount] = useState(0);

  return (
    <>
      <input
        id="roll-day-input"
        type="number"
        value={rollDay}
        onChange={(e) => setRollDay(Number(e.target.value))}
      />

      <button
        onClick={() => {
          console.log("Button clicked");
          setCount((c) => c + 1);
        }}
      >
        Count: {count}
      </button>
    </>
  );
};

export default App;
npm create vite@latest react-lag-test -- --template react-ts
cd react-lag-test
npm install
npm run dev

npm install -g @angular/cli

Create a new app:

ng new angular-lag-test

Choose:

CSS
No SSR

Run it:

cd angular-lag-test
ng serve
