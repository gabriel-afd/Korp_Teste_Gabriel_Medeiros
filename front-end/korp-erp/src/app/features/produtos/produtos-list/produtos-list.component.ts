import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProdutoService } from '../../../core/services/produto.service';
import { ProdutoFormComponent } from '../produto-form/produto-form.component';


@Component({
  selector: 'app-produtos-list',
  imports: [MatTableModule, MatButtonModule, MatProgressSpinnerModule, MatCardModule, MatDialogModule, MatPaginatorModule],
  templateUrl: './produtos-list.component.html',
  styleUrl: './produtos-list.component.scss'
})
export class ProdutosListComponent implements OnInit{

  produtoService = inject(ProdutoService);
  dialog = inject(MatDialog);
  platformId = inject(PLATFORM_ID);

  colunas = ['codigo', 'descricao', 'saldo'];
  paginaAtual = signal(1);
  tamanhoPagina = signal(10);

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
      this.carregar();
  }

  carregar() {
    this.produtoService.getPaged(this.paginaAtual(), this.tamanhoPagina()).subscribe();
  }

  onPageChange(event: PageEvent) {
    this.paginaAtual.set(event.pageIndex + 1);
    this.tamanhoPagina.set(event.pageSize);
    this.carregar();
  }

  abrirModal() {
    const ref = this.dialog.open(ProdutoFormComponent, { width: '800px' });
    ref.afterClosed().subscribe(criado => {
      if (criado) this.carregar();
    });
  }
}
