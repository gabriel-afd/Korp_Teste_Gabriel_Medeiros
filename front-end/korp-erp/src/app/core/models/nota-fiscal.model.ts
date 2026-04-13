export interface NotaFiscal {
  id: string;
  numero: number;
  status: 'Aberta' | 'Fechada';
  data: string;
  itens: ItemNota[];
}

export interface ItemNota {
  codigoProduto: string;
  quantidade: number;
}

export interface CriarNotaFiscalDto {
  itens: ItemNota[];
}
