import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';

//javascript code to bootstrap the appmodule

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
