import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateInvoiceRequest, Invoice } from '../models/invoice.model';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InvoicingService {

  private apiUrl = environment.invoicingApiUrl;

  constructor(private http: HttpClient) { }

  getInvoices(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.apiUrl);
  }

  createInvoice(request: CreateInvoiceRequest): Observable<Invoice> {
    return this.http.post<Invoice>(this.apiUrl, request);
  }

  printInvoice(id: number): Observable<any> {
    const headers = new HttpHeaders({
      'Idempotency-Key': crypto.randomUUID()
    });

    return this.http.post(`${this.apiUrl}/${id}/print`, {}, { headers });
  }
}
