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

<!DOCTYPE html>
<html>
<body>
  <input type="text">
  <button onclick="console.log('clicked')">Button</button>
</body>
</html>
