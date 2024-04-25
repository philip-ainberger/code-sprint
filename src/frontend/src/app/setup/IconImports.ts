import { ModuleWithProviders, Type } from "@angular/core";
import { NgIconsModule } from '@ng-icons/core';
import { octMarkGithub } from '@ng-icons/octicons';
import { heroArrowPathSolid, heroArrowTrendingDownSolid, heroArrowTrendingUpSolid, heroCalendarDaysSolid, heroCalendarSolid, heroCheckBadgeSolid, heroChevronLeftSolid, heroChevronRightSolid, heroChevronUpDownSolid, heroClockSolid, heroCodeBracketSolid, heroCogSolid, heroCommandLineSolid, heroFireSolid, heroInformationCircleSolid, heroLinkSolid, heroMoonSolid, heroPaperAirplaneSolid, heroPencilSolid, heroPlusSolid, heroPuzzlePieceSolid, heroSunSolid, heroTagSolid, heroTrashSolid, heroUserCircleSolid, heroVariableSolid, heroWrenchSolid, heroXMarkSolid } from '@ng-icons/heroicons/solid';

export function ImportIcons(): ModuleWithProviders<NgIconsModule>{
    return NgIconsModule.withIcons({ 
        heroUserCircleSolid, 
        heroMoonSolid, 
        heroSunSolid, 
        heroCodeBracketSolid, 
        heroPencilSolid, 
        heroArrowPathSolid,
        heroArrowTrendingDownSolid,
        heroArrowTrendingUpSolid,
        heroCheckBadgeSolid,
        heroCogSolid,
        heroFireSolid,
        heroLinkSolid,
        heroTagSolid,
        heroTrashSolid,
        heroVariableSolid,
        heroPuzzlePieceSolid,
        heroCommandLineSolid,
        heroClockSolid,
        heroCalendarDaysSolid,
        heroCalendarSolid,
        octMarkGithub,
        heroPlusSolid,
        heroXMarkSolid,
        heroWrenchSolid,
        heroInformationCircleSolid,
        heroPaperAirplaneSolid,
        heroChevronRightSolid,
        heroChevronLeftSolid,
        heroChevronUpDownSolid
     });
}