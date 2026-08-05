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

import { ProyectosListComponent } from './proyectos-list/proyectos-list.component';
import { ProyectoFormComponent } from './proyecto-form/proyecto-form.component';

const routes: Routes = [
    { path: '', component: ProyectosListComponent }
];

@NgModule({
    declarations: [
        ProyectosListComponent,
        ProyectoFormComponent
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
        ConfirmDialogModule
    ]
})
export class ProyectosModule { }
