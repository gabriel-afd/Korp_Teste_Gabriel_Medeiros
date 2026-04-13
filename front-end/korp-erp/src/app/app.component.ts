import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidenavComponent } from './shared/components/sidenav/sidenav.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { ProdutosListComponent } from './features/produtos/produtos-list/produtos-list.component';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SidenavComponent, MatSidenavModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'korp-erp';
}
