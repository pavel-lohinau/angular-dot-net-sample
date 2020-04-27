import { ValidatorFn, AbstractControl } from '@angular/forms';

export class CustomValidator {
    public static pattern(reg: RegExp): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } => {
            const value = <string>control.value;
            return value.match(reg) ? null : { 'pattern': { value } };
        };
    }
}
