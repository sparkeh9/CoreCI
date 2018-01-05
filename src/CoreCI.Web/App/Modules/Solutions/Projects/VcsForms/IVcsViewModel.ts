import { ControllerValidateResult } from 'aurelia-validation';
export interface IVcsViewModel
{
    validate(): Promise<ControllerValidateResult>;
}