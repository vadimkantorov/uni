/********************************************************************
 * qofmath128.h -- an 128-bit integer library                       *
 * Copyright (C) 2004 Linas Vepstas <linas@linas.org>               *
 *                                                                  *
 * This program is free software; you can redistribute it and/or    *
 * modify it under the terms of the GNU General Public License as   *
 * published by the Free Software Foundation; either version 2 of   *
 * the License, or (at your option) any later version.              *
 *                                                                  *
 * This program is distributed in the hope that it will be useful,  *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of   *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the    *
 * GNU General Public License for more details.                     *
 *                                                                  *
 * You should have received a copy of the GNU General Public License*
 * along with this program; if not, contact:                        *
 *                                                                  *
 * Free Software Foundation           Voice:  +1-617-542-5942       *
 * 51 Franklin Street, Fifth Floor    Fax:    +1-617-542-2652       *
 * Boston, MA  02110-1301,  USA       gnu@gnu.org                   *
 *                                                                  *
 *******************************************************************/

#ifndef QOF_MATH_128_H
#define QOF_MATH_128_H

#include "glib.h"



//gboolean equal128 (qofint128 a, qofint128 b);

int cmp128 (qofint128 a, qofint128 b);

qofint128 shift128 (qofint128 x);

qofint128 shiftleft128 (qofint128 x);

qofint128 inc128 (qofint128 a);

qofint128 add128 (qofint128 a, qofint128 b);

qofint128 mult128 (gint64 a, gint64 b);

qofint128 div128 (qofint128 n, gint64 d);

gint64 rem128 (qofint128 n, gint64 d);

guint64 gcf64(guint64 num, guint64 denom);

qofint128 lcm128 (guint64 a, guint64 b);

#endif
