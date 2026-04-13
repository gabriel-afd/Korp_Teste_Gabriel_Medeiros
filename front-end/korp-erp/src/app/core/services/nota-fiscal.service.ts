import { Injectable, signal, inject } from '@angular/core';
import { PagedResult } from '../models/paged-result.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, delay, finalize, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { NotaFiscal, CriarNotaFiscalDto } from '../models/nota-fiscal.model';

@Injectable({ providedIn: 'root' })
export class NotaFiscalService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.faturamentoApi}/notasfiscais`;

  notas = signal<NotaFiscal[]>([]);
  total = signal(0);
  totalPaginas = signal(0);
  imprimindo = signal<string | null>(null);
  loading = signal(false);

  getPaged(pagina: number = 1, tamanhoPagina: number = 10) {
    this.loading.set(true);
    const params = new HttpParams()
      .set('pagina', pagina)
      .set('tamanhoPagina', tamanhoPagina);

    return this.http.get<PagedResult<NotaFiscal>>(this.baseUrl, { params }).pipe(
      tap(result => {
        this.notas.set(result.items);
        this.total.set(result.total);
        this.totalPaginas.set(result.totalPaginas);
      }),
      finalize(() => this.loading.set(false))
    );
  }

  criar(dto: CriarNotaFiscalDto) {
    return this.http.post<NotaFiscal>(this.baseUrl, dto).pipe(
      tap(nova => this.notas.update(lista => [...lista, nova]))
    );
  }

  imprimir(id: string) {
    this.imprimindo.set(id);
    return this.http.post(`${this.baseUrl}/${id}/imprimir`, {}).pipe(
      delay(600),
      tap(() => {
        this.notas.update(lista =>
          lista.map(n => {
            if (n.id === id) {
              return { ...n, status: 'Fechada' as const };
            }
            return n;
          })
        );
      }),
      finalize(() => this.imprimindo.set(null))
    );
  }
}
