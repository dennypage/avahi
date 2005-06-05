# $Id: Makefile.am 90 2005-05-23 16:15:12Z lennart $
#
# This file is part of avahi.
# 
# avahi is free software; you can redistribute it and/or modify it
# under the terms of the GNU Lesser General Public License as
# published by the Free Software Foundation; either version 2 of the
# License, or (at your option) any later version.
#
# avahi is distributed in the hope that it will be useful, but WITHOUT
# ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
# or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
# License for more details.
#
# You should have received a copy of the GNU Lesser General Public
# License along with avahi; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
# USA.

AM_CFLAGS=-I$(top_srcdir) -D_GNU_SOURCE

# GLIB 2.0
AM_CFLAGS+=$(GLIB20_CFLAGS)
AM_LDADD=$(GLIB20_LIBS)

# This cool debug trap works on i386/gcc only
AM_CFLAGS+='-DDEBUG_TRAP=__asm__("int $$3")'

avahi_clientincludedir=$(includedir)/avahi-client

avahi_clientinclude_HEADERS =
	client.h

noinst_PROGRAMS = \
	client-test

lib_LTLIBRARIES = \
	libavahi-client.la 

libavahi_client_la_SOURCES = \
	client.c client.h

client_test_SOURCES = \
	client-test.c
client_test_CFLAGS = $(AM_CFLAGS)
client_test_LDADD = $(AM_LDADD) libavahi-client.la