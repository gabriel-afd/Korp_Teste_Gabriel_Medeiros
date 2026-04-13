export interface Produto {
  id: string;
  codigo: string;
  descricao: string;
  saldo: number;
}

export interface CriarProdutoDto {
  codigo: string;
  descricao: string;
  saldo: number;
}
