<input
  [value]="value"
  (input)="value = $any($event.target).value"
/>

<button (click)="count = count + 1">
  Count: {{ count }}
</button>
Replace app.component.ts with:
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.component.html'
})
export class AppComponent {
  value = '';
  count = 0;
}

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
