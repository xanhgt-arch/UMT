import { useMemo, useState } from "react";
import { Plus, Search, ShieldCheck, Trash2, X } from "lucide-react";
import { toast } from "sonner";

import { PageHeader } from "@/components/layout/PageHeader";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { AdminUserDialog } from "@/components/admin/AdminUserDialog";
import { shortDate } from "@/lib/format";
import { useAdminData } from "@/lib/admin-data";

export default function AdminsPage() {
  const { admins, removeAdmin } = useAdminData();

  const [query, setQuery] = useState("");
  const [dialogOpen, setDialogOpen] = useState(false);

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();

    if (!q) return admins;

    return admins.filter((a) =>
      [
        a.fullName,
        a.addedBy ?? "",
        a.addedOn ?? "",
      ]
        .join(" ")
        .toLowerCase()
        .includes(q)
    );
  }, [query, admins]);

  return (
    <div className="space-y-8">
      <PageHeader
        title="Admins"
        description="Manage UMT administrators."
        action={
          <Button
            className="gap-2 rounded-xl"
            onClick={() => setDialogOpen(true)}
          >
            <Plus className="size-4" />
            Add admin
          </Button>
        }
      />

      <Card className="max-w-4xl">
        <CardContent className="p-5">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <div className="relative w-full sm:max-w-sm">
              <Search className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />

              <Input
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Search admins by Admin ID…"
                className="h-10 rounded-xl pr-10 pl-9"
                aria-label="Search admins"
              />

              {query ? (
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  onClick={() => setQuery("")}
                  className="absolute top-1/2 right-1 size-8 -translate-y-1/2 rounded-lg text-muted-foreground hover:text-foreground"
                  aria-label="Clear admin search"
                >
                  <X className="size-4" />
                </Button>
              ) : null}
            </div>

            <div className="text-xs text-muted-foreground">
              {filtered.length} of {admins.length} admins
            </div>
          </div>

          <div className="mt-4 overflow-hidden rounded-xl border border-border">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/40 hover:bg-muted/40">
                  <TableHead>Admin ID</TableHead>
                  <TableHead>Added by</TableHead>
                  <TableHead>Added on</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>

              <TableBody>
                {filtered.map((a) => (
                  <TableRow key={a.id}>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <ShieldCheck className="size-4 text-muted-foreground" />

                        <div className="leading-tight">
                          <div className="font-medium">{a.fullName}</div>

                          <div className="text-xs text-muted-foreground">
                            {a.fullName?.toLowerCase()}@cooperstandard.com
                          </div>
                        </div>
                      </div>
                    </TableCell>

                    <TableCell className="text-sm">
                      {a.addedBy ?? "—"}
                    </TableCell>

                    <TableCell className="text-sm text-muted-foreground">
                      {a.addedOn ? shortDate(a.addedOn) : "—"}
                    </TableCell>

                    <TableCell className="text-right">
                      <Button
                        variant="ghost"
                        size="icon"
                        className="size-8 rounded-lg text-destructive hover:bg-destructive/10 hover:text-destructive"
                        aria-label={`Delete ${a.fullName}`}
                        onClick={async () => {
                          await fetch(`/api/admin/${a.fullName}`, {
                            method: "DELETE",
                            credentials: "include",
                          });

                          removeAdmin(a.id);

                          toast.success(`${a.fullName} removed.`);
                        }}
                      >
                        <Trash2 className="size-4" />
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}

                {filtered.length === 0 ? (
                  <TableRow>
                    <TableCell
                      colSpan={4}
                      className="py-12 text-center text-sm text-muted-foreground"
                    >
                      {admins.length === 0
                        ? "No admins yet. Click “Add admin” to create the first one."
                        : "No admins match your search."}
                    </TableCell>
                  </TableRow>
                ) : null}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      <AdminUserDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
      />

      <Card className="max-w-4xl border-dashed bg-muted/30">
        <CardContent className="flex items-center gap-3 p-5">
          <ShieldCheck className="size-5 text-primary" />

          <div className="text-sm text-muted-foreground">
            Admins have elevated access to manage VDI users, domains, and other
            administrative settings. Add new admins sparingly.
          </div>
        </CardContent>
      </Card>
    </div>
  );
}