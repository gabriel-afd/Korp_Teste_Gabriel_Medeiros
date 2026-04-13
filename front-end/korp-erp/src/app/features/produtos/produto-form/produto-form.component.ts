import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ProdutoService } from '../../../core/services/produto.service';

@Component({
  selector: 'app-produto-form',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './produto-form.component.html',
  styleUrl: './produto-form.component.scss'
})
export class ProdutoFormComponent {

  fb = inject(FormBuilder);
  dialogRef = inject(MatDialogRef<ProdutoFormComponent>);
  produtoService = inject(ProdutoService);

  form = this.fb.group({
    codigo: ['', Validators.required],
    descricao: ['', Validators.required],
    saldo: [0, [Validators.required, Validators.min(0)]]
  });

  salvar() {
    if (this.form.invalid) return;

    this.produtoService.criar(this.form.value as any).subscribe({
      next: () => this.dialogRef.close(true),
      error: () => {}
    });
  }

  fechar() {
    this.dialogRef.close(false);
  }

}
