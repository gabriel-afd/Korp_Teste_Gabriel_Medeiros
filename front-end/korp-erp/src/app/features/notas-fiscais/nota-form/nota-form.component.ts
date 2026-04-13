import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { NotaFiscalService } from '../../../core/services/nota-fiscal.service';
import { ProdutoService } from '../../../core/services/produto.service';
import { Produto } from '../../../core/models/produto.model';

@Component({
  selector: 'app-nota-form',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatTableModule
  ],
  templateUrl: './nota-form.component.html',
  styleUrl: './nota-form.component.scss'
})
export class NotaFormComponent implements OnInit{

  fb = inject(FormBuilder);
  dialogRef = inject(MatDialogRef<NotaFormComponent>);
  notaService = inject(NotaFiscalService);
  produtoService = inject(ProdutoService);

  produtos = signal<Produto[]>([]);

  itens = signal<{ codigoProduto: string; quantidade: number }[]>([]);

  itemForm = this.fb.group({
    codigoProduto: ['', Validators.required],
    quantidade: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnInit() {
    this.produtoService.getPaged(1, 100).subscribe(() => {
      this.produtos.set(this.produtoService.produtos());
    });
  }

  adicionarItem() {
    if (this.itemForm.invalid) return;

    const { codigoProduto, quantidade } = this.itemForm.value;

    const jaExiste = this.itens().some(i => i.codigoProduto === codigoProduto);
    if (jaExiste) return;

    this.itens.update(lista => [
      ...lista,
      { codigoProduto: codigoProduto!, quantidade: quantidade! }
    ]);

    this.itemForm.reset({ codigoProduto: '', quantidade: 1 });
  }

  removerItem(codigo: string) {
    this.itens.update(lista => lista.filter(i => i.codigoProduto !== codigo));
  }

  descricaoProduto(codigo: string): string {
    return this.produtos().find(p => p.codigo === codigo)?.descricao ?? codigo;
  }

  salvar() {
    if (this.itens().length === 0) return;

    this.notaService.criar({ itens: this.itens() }).subscribe({
      next: () => this.dialogRef.close(true),
      error: () => {}
    });
  }

  fechar() {
    this.dialogRef.close(false);
  }
}
