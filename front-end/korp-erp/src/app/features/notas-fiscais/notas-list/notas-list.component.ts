import { DatePipe, isPlatformBrowser } from '@angular/common';
import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NotaFiscalService } from '../../../core/services/nota-fiscal.service';
import { NotaFormComponent } from '../nota-form/nota-form.component';

@Component({
  selector: 'app-notas-list',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatIconModule,
    MatTooltipModule,
    DatePipe
  ],
  templateUrl: './notas-list.component.html',
  styleUrl: './notas-list.component.scss'
})
export class NotasListComponent implements OnInit{

  notaService = inject(NotaFiscalService);
  dialog = inject(MatDialog);
  platformId = inject(PLATFORM_ID);
  isBrowser = isPlatformBrowser(this.platformId);


  colunas = ['numero', 'data', 'status', 'acoes'];
  paginaAtual = signal(1);
  tamanhoPagina = signal(5);

  ngOnInit(): void {
    if (!this.isBrowser) return;
    this.carregar();
  }

  carregar() {
    this.notaService.getPaged(this.paginaAtual(), this.tamanhoPagina()).subscribe();
  }

  onPageChange(event: PageEvent) {
    this.paginaAtual.set(event.pageIndex + 1);
    this.tamanhoPagina.set(event.pageSize);
    this.carregar();
  }

  abrirModal() {
    const ref = this.dialog.open(NotaFormComponent, { width: '800px' });
    ref.afterClosed().subscribe(criado => {
      if (criado) this.carregar();
    });
  }

  imprimir(id: string) {
    this.notaService.imprimir(id).subscribe({
      error: () => {}
    });
  }

}
