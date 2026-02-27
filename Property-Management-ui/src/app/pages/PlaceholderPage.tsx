import { Construction } from 'lucide-react';

interface PlaceholderPageProps {
  title: string;
  description: string;
}

export function PlaceholderPage({ title, description }: PlaceholderPageProps) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-[#13161f]">
      <div className="text-center">
        <div className="w-24 h-24 bg-blue-600/20 rounded-full flex items-center justify-center mx-auto mb-6">
          <Construction className="w-12 h-12 text-blue-500" />
        </div>
        <h1 className="text-white text-3xl font-bold mb-3">{title}</h1>
        <p className="text-gray-400 text-lg">{description}</p>
      </div>
    </div>
  );
}
