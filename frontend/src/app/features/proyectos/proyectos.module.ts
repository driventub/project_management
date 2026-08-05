import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';

import { ProyectosListComponent } from './proyectos-list/proyectos-list.component';
import { ProyectoFormComponent } from './proyecto-form/proyecto-form.component';
import { ColumnasListComponent } from './columnas/columnas-list/columnas-list.component';
import { ColumnaFormComponent } from './columnas/columna-form/columna-form.component';

const routes: Routes = [
    { path: '', component: ProyectosListComponent },
    { path: ':id/columnas', component: ColumnasListComponent }
];

@NgModule({
    declarations: [
        ProyectosListComponent,
        ProyectoFormComponent,
        ColumnasListComponent,
        ColumnaFormComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild(routes),
        ButtonModule,
        TableModule,
        InputTextModule,
        InputTextareaModule,
        DialogModule,
        DropdownModule,
        CalendarModule,
        ToastModule,
        ConfirmDialogModule,
        TooltipModule
    ]
})
export class ProyectosModule { }
