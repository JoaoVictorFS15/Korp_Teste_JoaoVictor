import { Routes } from '@angular/router';
import { ProductsComponent } from './components/products/products.component';
import { InvoicesComponent } from './components/invoices/invoices.component';

export const routes: Routes = [
    { path: 'products', component: ProductsComponent },
    { path: 'invoices', component: InvoicesComponent },
    { path: '', redirectTo: '/products', pathMatch: 'full' }
];
