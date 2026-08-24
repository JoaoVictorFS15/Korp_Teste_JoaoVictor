import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Invoice } from '../../models/invoice.model';
import { Product } from '../../models/product.model';
import { InvoicingService } from '../../services/invoicing.service';
import { StockService } from '../../services/stock.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.scss'
})
export class InvoicesComponent {

  invoices: Invoice[] = [];
  products: Product[] = [];

  newItems: { productId: number | null, quantity: number }[] = [];

  constructor(
    private invoicingService: InvoicingService,
    private stockService: StockService
  ) { }

  ngOnInit(): void {
    this.loadInvoices();
    this.loadProducts();
    this.addNewItemRow();
  }

  loadInvoices(): void {
    this.invoicingService.getInvoices().subscribe({
      next: (data) => this.invoices = data,
      error: (err) => console.error('Erro ao carregar notas', err)
    });
  }

  loadProducts(): void {
    this.stockService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: (err) => console.error('Erro ao carregar produtos', err)
    });
  } 

 
  addNewItemRow(): void {
    this.newItems.push({ productId: null, quantity: 1 });
  }

  removeItemRow(index: number): void {
    this.newItems.splice(index, 1);
    if (this.newItems.length === 0) this.addNewItemRow();
  }

  createInvoice(form: NgForm): void {
   
    const validItems = this.newItems.filter(i => i.productId != null && i.quantity > 0);

    if (validItems.length === 0) {
      Swal.fire('Atenção', 'Selecione pelo menos um produto para gerar a nota.', 'warning');
      return;
    }
   
    this.invoicingService.createInvoice({ items: validItems as any }).subscribe({
      next: (created) => {
        this.invoices.push(created);

        this.newItems = [];
        this.addNewItemRow();

        Swal.fire({ title: 'Nota Gerada!', text: 'Criada com status Aberta.', icon: 'success' });
      },
      error: (err) => Swal.fire('Erro', 'Falha ao criar nota fiscal.', 'error')
    });
  }

  printInvoice(id: number): void {

    Swal.fire({
      title: 'Imprimindo e Baixando Estoque...',
      text: 'Aguardando serviço de estoque...',
      allowOutsideClick: false,
      didOpen: () => Swal.showLoading()
    });
    this.invoicingService.printInvoice(id).subscribe({
      next: (res) => {
        Swal.fire('Impresso!', res.message, 'success');
        this.loadInvoices(); 
      },
      error: (err) => {     
        let msg = err.error?.message || 'Erro na comunicação com Estoque.';
        msg = msg.replace(/produto (\d+)/, (textoOriginal: string, idEncontrado: string) => {
          const produtoReal = this.products.find(p => p.id === Number(idEncontrado));
          // Se achar o produto na memória, mostra o código e a descrição. Se não achar, mantém original.
          return produtoReal ? `produto ${produtoReal.code} (${produtoReal.description})` : textoOriginal;
        });
        Swal.fire('Não foi possível imprimir', msg, 'error');
      }
    });
  }

  getTotalQuantity(invoice: Invoice): number {
    if (!invoice.items) return 0;
    return invoice.items.reduce((total, item) => total + item.quantity, 0);
  }
}
