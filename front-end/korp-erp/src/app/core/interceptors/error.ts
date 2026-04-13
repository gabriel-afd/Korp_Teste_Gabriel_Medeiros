import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError(error => {
      let message = 'Erro inesperado. Tente novamente.';

      if (error.status === 400)
        message = error.error?.error ?? 'Requisição inválida.';
      else if (error.status === 503)
        message = 'Serviço de Estoque indisponível. Tente novamente em instantes.';
      else if (error.status === 0)
        message = 'Sem conexão com o servidor.';

      snackBar.open(message, 'Fechar', {
        duration: 5000,
        panelClass: ['snack-error']
      });

      return throwError(() => error);
    })
  );
};
