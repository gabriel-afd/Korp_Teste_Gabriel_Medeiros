import { HttpClient, HttpParams } from '@angular/common/http';
import { PagedResult } from '../models/paged-result.model';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CriarProdutoDto, Produto } from '../models/produto.model';
import { catchError, finalize, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {

  private http = inject(HttpClient);
  private baseUrl = `${environment.estoqueApi}/produtos`


  produtos = signal<Produto[]>([]);
  total = signal(0);
  totalPaginas = signal(0);
  loading = signal(false);

  getPaged(pagina: number = 1, tamanhoPagina: number = 10) {
    this.loading.set(true);
    const params = new HttpParams()
      .set('pagina', pagina)
      .set('tamanhoPagina', tamanhoPagina);

    return this.http.get<PagedResult<Produto>>(this.baseUrl, { params }).pipe(
      tap(result => {
        this.produtos.set(result.items);
        this.total.set(result.total);
        this.totalPaginas.set(result.totalPaginas);
      }),
      finalize(() => this.loading.set(false))
    );
  }

  criar(dto: CriarProdutoDto){
    return this.http.post<Produto>(this.baseUrl, dto).pipe(
      tap(novo => {
        this.produtos.update(lista => [...lista, novo]);
      }),
      catchError(err => {
        console.error('Erro ao criar produto', err);
        return throwError(() => err);
      })
    );
  }
}
