import 'zone.js'
import {bootstrapApplication} from '@angular/platform-browser'
import WindowFeatureTestPanel from './generated/WindowFeatureTestPanel'
import '../../InfiniAutomationTests.WebApp/src/style.css'

bootstrapApplication(WindowFeatureTestPanel).catch(error => console.error(error))
