import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { BistroHeader } from "@/components/BistroHeader";
import { Card } from "@/components/ui/card";
import {
  buscarVendas,
  formatBRL,
  type VendasHistoricos,
} from "@/lib/bistro-store";
import { toast } from "sonner";

export const Route = createFileRoute("/vendas")({
  head: () => ({
    meta: [{ title: "Vendas — BC Bistro" }],
  }),

  component: Vendas,
});

function Vendas() {
  const [vendas, setVendas] = useState<VendasHistoricos[]>([]);

  useEffect(() => {
    loadVendas();
  }, []);

  async function loadVendas() {
    try {
      const dados = await buscarVendas();

      setVendas(dados);
    } catch {
      toast.error("Erro ao carregar vendas");
    }
  }

  const resumo = vendas[0];

  return (
    <div className="min-h-screen bg-background">
      <BistroHeader />

      <main className="mx-auto max-w-6xl px-4 py-8">
        <h1 className="mb-6 font-display text-4xl font-bold">
          Vendas
        </h1>

        <div className="mb-8 grid gap-4 sm:grid-cols-3">
          <StatCard
            label="Vendas hoje"
            value={String(resumo?.vendasTotaisHoje ?? 0)}
          />

          <StatCard
            label="Total vendido hoje"
            value={formatBRL(resumo?.valorVendidoHoje ?? 0)}
            highlight
          />

          <StatCard
            label="Total geral"
            value={formatBRL(resumo?.valorVendidoGeral ?? 0)}
          />
        </div>

        <Card className="overflow-hidden">
          <div className="border-b border-border bg-foreground px-5 py-3 font-display text-lg text-primary-foreground">
            Histórico de vendas
          </div>

          {vendas.length === 0 ? (
            <p className="px-5 py-10 text-center text-muted-foreground">
              Nenhuma venda registrada ainda.
            </p>
          ) : (
            <ul className="divide-y divide-border">
              {vendas.map((v) => {
                const d = new Date(v.dataVenda);

                return (
                  <li
                    key={v.id}
                    className="px-5 py-4"
                  >
                    <div className="flex flex-wrap items-center justify-between gap-2">
                      <div>
                        <div className="text-sm text-muted-foreground">
                          {d.toLocaleDateString("pt-BR")} às{" "}
                          {d.toLocaleTimeString("pt-BR", {
                            hour: "2-digit",
                            minute: "2-digit",
                          })}
                        </div>

                        <div className="font-display text-lg">
                          Venda #{v.id}
                        </div>
                      </div>

                      <div className="text-right">
                        <div className="text-xs text-muted-foreground">
                          Total vendido hoje
                        </div>

                        <div className="font-display text-xl font-bold text-[color:var(--gold)]">
                          {formatBRL(v.valorVendidoHoje)}
                        </div>
                      </div>
                    </div>
                  </li>
                );
              })}
            </ul>
          )}
        </Card>
      </main>
    </div>
  );
}

function StatCard({
  label,
  value,
  highlight,
}: {
  label: string;
  value: string;
  highlight?: boolean;
}) {
  return (
    <Card
      className={`p-5 ${
        highlight
          ? "border-[color:var(--gold)] bg-[color:var(--gold-soft)]/40"
          : ""
      }`}
    >
      <div className="text-xs uppercase tracking-widest text-muted-foreground">
        {label}
      </div>

      <div className="mt-1 font-display text-3xl font-bold">
        {value}
      </div>
    </Card>
  );
}