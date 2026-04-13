export interface NotaFiscal {
  id: string;
  numero: number;
  status: 'Aberta' | 'Fechada';
  data: string;
  itens: ItemNota[];
}

export interface ItemNota {
  codigoProduto: string;
  descricao?: string;
  descricaoProduto?: string;
  quantidade: number;
}

export interface CriarItemNotaDto {
  codigoProduto: string;
  descricao: string;
  quantidade: number;
}

export interface CriarNotaFiscalDto {
  itens: CriarItemNotaDto[];
}
