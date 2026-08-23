export interface InvoiceItem {
  productId: number;
  quantity: number;
}
export interface Invoice {
  id: number;
  sequentialNumber: number;
  status: string;
  createdAt: string;
  items: InvoiceItem[];
}
export interface CreateInvoiceRequest {
  items: InvoiceItem[];
}
