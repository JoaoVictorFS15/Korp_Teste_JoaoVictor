import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Product } from '../../models/product.model';
import { StockService } from '../../services/stock.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent {

  products: Product[] = [];

  newProduct: Product = {
    code: '',
    description: '',
    balance: 0
  };

  constructor(private stockService: StockService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.stockService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: (err) => console.error('Erro ao carregar produtos', err)
    });
  }

  createProduct(form: NgForm): void {
    this.stockService.createProduct(this.newProduct).subscribe({
      next: (createdProduct) => {
        this.products.push(createdProduct);
        form.resetForm({ balance: 0 });
        this.newProduct = { code: '', description: '', balance: 0 };
        Swal.fire({
          title: 'Sucesso!',
          text: 'Produto cadastrado com sucesso no estoque.',
          icon: 'success',
          confirmButtonColor: '#198754',
          confirmButtonText: 'OK'
        });
      },
      error: (err) => {
        Swal.fire({
          title: 'Ops!',
          text: 'Ocorreu um erro ao cadastrar o produto.',
          icon: 'error',
          confirmButtonColor: '#dc3545', 
          confirmButtonText: 'Tentar Novamente'
        });
      }
    });
  }

}
